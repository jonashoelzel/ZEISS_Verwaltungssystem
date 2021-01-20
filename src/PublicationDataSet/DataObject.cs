﻿namespace PublicationDataSet
{
    public class DataObject : IDataObject
    {
        private int? _ID;
        public int? ID
        {
            get { return _ID; }
            set
            {
                if (_ID is null)
                    _ID = ID;
            }
        }
    }
}