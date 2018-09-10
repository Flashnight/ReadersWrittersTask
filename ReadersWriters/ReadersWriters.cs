using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ReadersWriters
{
    class ReadersWriters
    {
        private Mutex _temp;
        private Mutex[] _mutexes;
        private Thread[] _readers;
        private Thread[] _writters;
        private int _counterWriters;
        private int _counterReaders;

        private string[] _fileNames;
        private string[] _str;
        
        public ReadersWriters()
        {
            _temp = null;
            _mutexes = new Mutex[3];
            _readers = new Thread[3];
            _writters = new Thread[3];
            _counterWriters = 0;
            _counterReaders = 0;

            _fileNames = new string[3] { "file1", "file2", "file3" };
            _str = new string[3] { "АБВГД", "ABCDE", "12345" };
        }

        public void Proccess()
        {
            _temp = new Mutex();
            for (var i = 0; i < 3; i++)
            {
                _mutexes[i] = new Mutex();
                _readers[i] = new Thread(ThreadWritter);
                _readers[i].Start();
                _writters[i] = new Thread(ThreadReader);
                _writters[i].Start();
            }

            Console.ReadKey();
        }

        private void ThreadWritter()
        {
            var index = 0;
            _temp.WaitOne();
            index = _counterWriters;
            _counterWriters++;
            _temp.ReleaseMutex();
            Console.WriteLine($"Writer {_counterWriters}");

            var counter = 0;

            while (true)
            {
                _mutexes[index].WaitOne();

                using (var writter = new StreamWriter(_fileNames[index]))
                {
                    writter.WriteLine(_str[index]);
                }

                _mutexes[index].ReleaseMutex();
                Thread.Sleep(1500);
            }

            Console.WriteLine("$Write {index}");
        }

        private void ThreadReader()
        {
            var index = 0;
            _temp.WaitOne();
            index = _counterReaders;
            _counterReaders++;
            _temp.ReleaseMutex();

            var counter = 0;

            while (true)
            {
                _mutexes[index].WaitOne();

                var str = "";

                using (var reader = new StreamReader(_fileNames[index]))
                {
                    while (reader.EndOfStream == false)
                    {
                        str += reader.ReadLine();
                    }
                }

                Console.WriteLine($"{str} {counter}");
                counter++;
                _mutexes[index].ReleaseMutex();

                Thread.Sleep(1500);
            }
        }
    }
}
