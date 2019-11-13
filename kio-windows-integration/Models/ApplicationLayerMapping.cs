namespace kio_windows_integration.Models
{
    /// <summary>
    /// Represents an application mapping wih a keyboard layer.
    /// </summary>
    public class ApplicationLayerMapping
    {
        public string ProcessName { get; set; }
        public int Layer { get; set; }

        public ApplicationLayerMapping()
        {
            ProcessName = "";
            Layer = -1;
        }

        public ApplicationLayerMapping(string processName, int layer)
        {
            ProcessName = processName;
            Layer = layer;
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