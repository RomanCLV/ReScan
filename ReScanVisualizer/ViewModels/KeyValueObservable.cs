using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.ViewModels
{
    public class KeyValueObservable<T, V> : ViewModelBase
        where T : ViewModelBase
        where V : ViewModelBase
    {
        private T _key;
        public T Key
        {
            get => _key;
            private set
            {
                if (_key is null)
                {
                    SetValue(ref _key, value);
                }
                else
                {
                    if (!_key.Equals(value))
                    {
                        _key.Dispose();
                        SetValue(ref _key, value);
                    }
                }
            }
        }

        private V _value;
        public V Value
        {
            get => _value;
            set
            {
                if (_value is null)
                {
                    SetValue(ref _value, value);
                }
                else
                {
                    if (!_value.Equals(value))
                    {
                        _value.Dispose();
                        SetValue(ref _value, value);
                    }
                }
            }
        }

        public KeyValueObservable(T key) : this(key, null)
        {
        }

        public KeyValueObservable(T key, V value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _key = key;
            _value = value;
        }
    }
}
