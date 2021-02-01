namespace Codefusion.Jaskier.API
{
    public class CommitsRange
    {
        public CommitsRange(string commitFrom, string commitTo)
        {
            this.CommitFrom = commitFrom;
            this.CommitTo = commitTo;
        }

        public CommitsRange()
        {            
        }

        public string CommitFrom { get; set; }

        public string CommitTo { get; set; }  
    }
}
