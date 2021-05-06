using System;
using System.Collections.Generic;
using System.Threading;

namespace Server
{
    class QueryReader
    {
        List<String> messageQuery;
        List<TCPChat> listenerList;
        Boolean done;
        Mutex mutex;

        public QueryReader(List<String> query, List<TCPChat> listeners, Mutex mut)
        {
            messageQuery = query;
            listenerList = listeners;
            mutex = mut;
            done = false;
        }

        public void Run()
        {
            while (done == false)
            {
                mutex.WaitOne();
                for (int i = 0; i < listenerList.Count; i++)
                {
                    if (listenerList[i].Connected == false)
                    {
                        listenerList.RemoveAt(i);
                    }
                }

                if (messageQuery.Count > 0)
                {
                    foreach (TCPChat listener in listenerList)
                    {
                        listener.send_Msg(messageQuery[0]);
                    }
                    messageQuery.RemoveAt(0);
                    Thread.Sleep(10);
                }
                mutex.ReleaseMutex();
            }
        }
    }
}
