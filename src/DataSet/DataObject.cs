using System;

namespace Zeiss.PublicationManager.Data.DataSet
{
    public class DataObject : IDataObject
    {
        private Guid? _ID;
        public Guid? ID
        {
            get { return _ID; }
            set
            {
                if (_ID is null)
                    _ID = value;
                else if (value is null)
                    _ID = null;
            }
        }

        public void ResetID()
        {
            _ID = null;
        }
    }
}