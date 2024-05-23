using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealDream
{
    public class EditorCoroutineUtil : MonoBehaviour
    {
#if UNITY_EDITOR
        private static List<EditorCoroutine> coroutines = new List<EditorCoroutine>();
#endif
        public static void StartCoroutine(IEnumerator routine)
        {
#if UNITY_EDITOR
            coroutines.Add(new EditorCoroutine(routine));
#endif
        }

        public static void StopAllCoroutine()
        {
#if UNITY_EDITOR
            foreach (var cor in coroutines)
            {
                cor.Stop();
            }
            coroutines.Clear();
#endif
        }

        public sealed class WaitForSeconds : YieldInstruction
        {
            public float WaitTime;

            /// <summary>
            ///   <para>Suspends the coroutine execution for the given amount of seconds using scaled time.</para>
            /// </summary>
            /// <param name="seconds">Delay execution by the amount of time in seconds.</param>
            public WaitForSeconds(float seconds) => this.WaitTime = seconds;
        }
        
        public class EditorCoroutine
        {
            internal EditorCoroutine(IEnumerator routine)
            {
#if UNITY_EDITOR
                m_Owner = null;
                m_Routine = routine;
                UnityEditor.EditorApplication.update += MoveNext;
#endif
            }
#if UNITY_EDITOR
            private struct YieldProcessor
            {
                enum DataType : byte
                {
                    None = 0,
                    WaitForSeconds = 1,
                    EditorCoroutine = 2,
                    AsyncOP = 3,
                }

                struct ProcessorData
                {
                    public DataType type;
                    public double targetTime;
                    public object current;
                }

                ProcessorData data;

                public void Set(object yield)
                {
                    if (yield == data.current)
                        return;

                    var type = yield.GetType();
                    var dataType = DataType.None;
                    double targetTime = -1;

                    if (type == typeof(WaitForSeconds))
                    {
                        targetTime = EditorApplication.timeSinceStartup + ((WaitForSeconds)yield).WaitTime;
                        dataType = DataType.WaitForSeconds;
                    }
                    else if (type == typeof(EditorCoroutine))
                    {
                        dataType = DataType.EditorCoroutine;
                    }
                    else if (type == typeof(AsyncOperation) || type.IsSubclassOf(typeof(AsyncOperation)))
                    {
                        dataType = DataType.AsyncOP;
                    }

                    data = new ProcessorData { current = yield, targetTime = targetTime, type = dataType };
                }

                public bool MoveNext(IEnumerator enumerator)
                {
                    bool advance = false;
                    switch (data.type)
                    {
                        case DataType.WaitForSeconds:
                            advance = data.targetTime <= EditorApplication.timeSinceStartup;
                            break;
                        case DataType.EditorCoroutine:
                            advance = (data.current as EditorCoroutine).m_IsDone;
                            break;
                        case DataType.AsyncOP:
                            advance = (data.current as AsyncOperation).isDone;
                            break;
                        default:
                            advance = data.current ==
                                      enumerator
                                          .Current; //a IEnumerator or a plain object was passed to the implementation
                            break;
                    }

                    if (advance)
                    {
                        data = default(ProcessorData);
                        return enumerator.MoveNext();
                    }

                    return true;
                }
            }

            internal WeakReference m_Owner;
            IEnumerator m_Routine;
            YieldProcessor m_Processor;

            bool m_IsDone;


            internal EditorCoroutine(IEnumerator routine, object owner)
            {
                m_Processor = new YieldProcessor();
                m_Owner = new WeakReference(owner);
                m_Routine = routine;
                EditorApplication.update += MoveNext;
            }

            internal void MoveNext()
            {
                if (m_Owner != null && !m_Owner.IsAlive)
                {
                    EditorApplication.update -= MoveNext;
                    return;
                }

                bool done = ProcessIEnumeratorRecursive(m_Routine);
                m_IsDone = !done;

                if (m_IsDone)
                    EditorApplication.update -= MoveNext;
            }

            static Stack<IEnumerator> kIEnumeratorProcessingStack = new Stack<IEnumerator>(32);

            private bool ProcessIEnumeratorRecursive(IEnumerator enumerator)
            {
                var root = enumerator;
                while (enumerator.Current as IEnumerator != null)
                {
                    kIEnumeratorProcessingStack.Push(enumerator);
                    enumerator = enumerator.Current as IEnumerator;
                }

                //process leaf
                m_Processor.Set(enumerator.Current);
                var result = m_Processor.MoveNext(enumerator);

                while (kIEnumeratorProcessingStack.Count > 1)
                {
                    if (!result)
                    {
                        result = kIEnumeratorProcessingStack.Pop().MoveNext();
                    }
                    else
                        kIEnumeratorProcessingStack.Clear();
                }

                if (kIEnumeratorProcessingStack.Count > 0 && !result && root == kIEnumeratorProcessingStack.Pop())
                {
                    result = root.MoveNext();
                }

                return result;
            }

            internal void Stop()
            {
                m_Owner = null;
                m_Routine = null;
                EditorApplication.update -= MoveNext;
            }
#endif
        }
    }
}