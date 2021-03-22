namespace Zeiss.Data.DataSet
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