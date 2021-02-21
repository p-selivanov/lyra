namespace Lyra.Contracts
{
    public class CustomerChangedEvent
    {
        public int CustomerId { get; set; }

        public int Balance { get; set; }

        public int TargetEntityVersion { get; set; }
    }
}
