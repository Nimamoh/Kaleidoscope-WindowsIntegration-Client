namespace kaleidoscope_companion.Models
{
    /// <summary>
    /// Represents an application mapping wih a keyboard layer.
    /// </summary>
    public class ApplicationLayerMapping
    {
        /// <summary>
        /// Optional executable path corresponding to the process.
        /// This property should only be used to compute additionnal informations
        /// about te mapping, it is not mandatory.
        /// </summary>
        public string ExePath { get; set; }
        public string ProcessName { get; set; }
        public int Layer { get; set; }

        public ApplicationLayerMapping()
        {
            ExePath = "";
            ProcessName = "";
            Layer = -1;
        }

        public ApplicationLayerMapping(string processName, int layer, string exePath = "")
        {
            ProcessName = processName;
            Layer = layer;
            ExePath = exePath;
        }

        protected bool Equals(ApplicationLayerMapping other)
        {
            return ProcessName == other.ProcessName && Layer == other.Layer;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ApplicationLayerMapping) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ProcessName != null ? ProcessName.GetHashCode() : 0) * 397) ^ Layer;
            }
        }
    }
}