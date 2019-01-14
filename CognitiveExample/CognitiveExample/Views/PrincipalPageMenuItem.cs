using System;

namespace CognitiveExample.Views
{

    public class PrincipalPageMenuItem
    {
        public PrincipalPageMenuItem()
        {
            TargetType = typeof(PrincipalPageDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}