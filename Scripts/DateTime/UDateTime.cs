using System;
using System.Globalization;
using UnityEngine;

namespace MustHave.UI
{
    // we have to use UDateTime instead of DateTime on our classes
    // we still typically need to either cast this to a DateTime or read the DateTime field directly
    [System.Serializable]
    public class UDateTime : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private DateTime dateTime;

        // if you don't want to use the PropertyDrawer then remove HideInInspector here
        [SerializeField, HideInInspector] private string _dateTime;

        public static implicit operator DateTime(UDateTime udt)
        {
            return udt.dateTime;
        }

        public static implicit operator UDateTime(DateTime dt)
        {
            return new UDateTime() { dateTime = dt };
        }

        public DateTime DateTime => dateTime;

        public bool TryParseDateTime()
        {
            if (TryParseDateTime(out DateTime parsedDateTime))
            {
                dateTime = parsedDateTime;
                return true;
            }
            return false;
        }

        public bool TryParseDateTime(out DateTime parsedDateTime)
        {
            return DateTime.TryParseExact(_dateTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);
        }

        public void OnAfterDeserialize()
        {
            TryParseDateTime(out dateTime);
        }

        public void OnBeforeSerialize()
        {
            _dateTime = dateTime.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public void SetDateTime(DateTime dateTime)
        {
            this.dateTime = dateTime;
        }
    }
}