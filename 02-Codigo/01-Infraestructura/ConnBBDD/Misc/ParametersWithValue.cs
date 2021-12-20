using System;

namespace ConnBBDD.Misc
{
    /// <summary>
    /// Objeto parametros y valor para sentencias INSERT
    /// </summary>
    [Serializable()]
    public partial class ParametersWithValue
    {
        public string sParam { get; set; }
        public object sValue { get; set; }
    }
}
