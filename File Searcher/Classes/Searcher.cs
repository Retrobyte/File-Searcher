using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace File_Searcher.Classes
{
    public class Searcher
    {
        private bool _searching;
        private string _contains;
        private int _track;
        private bool _caseSensitive;
        private object _lockObject;
        private List<string> _files;
        private ReturnCallback _callback;
        public delegate void ReturnCallback(string[] files);

        public Searcher(ReturnCallback callback)
        {
            _searching = false;
            _contains = string.Empty;
            _track = 0;
            _lockObject = new object();
            _files = null;
            _callback = callback;
        }

        public void Search(string path, string contains, bool caseSense = false)
        {
            if (_searching) return;

            _searching = true;
            _contains = contains;
            _track = 0;
            _caseSensitive = caseSense;
            _files = new List<string>();

            Thread t = new Thread(new ParameterizedThreadStart(initialize));
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start(path);
        }

        private void recSearch(object data)
        {
            string path = (string)data;

            try
            {
                foreach (string relPaths in Directory.GetDirectories(path))
                    process(relPaths);
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message); 
            }

            try
            {
                lock (_files)
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        if (_caseSensitive)
                        {
                            if (_contains == string.Empty || File.ReadAllText(file).Contains(_contains))
                                _files.Add(file);
                        }
                        else
                        {
                            if (_contains == string.Empty || File.ReadAllText(file).ToLower().Contains(_contains.ToLower()))
                                _files.Add(file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }

            Interlocked.Decrement(ref _track);

            lock (_lockObject)
            {
                if (_track == 0)
                {
                    _searching = false;

                    if (_callback != null) 
                        lock (_files) 
                            _callback(_files.ToArray());
                }
            }
        }

        private void initialize(object data)
        {
            process((string)data);
        }

        private void process(string path)
        {
            Interlocked.Increment(ref _track);
            
            if (!ThreadPool.QueueUserWorkItem(recSearch, path)) 
                recSearch(path);
        }
    }
}
