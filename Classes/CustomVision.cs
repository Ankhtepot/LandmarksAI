using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandmarksAI.Classes
{
    class CustomVision
    {       
        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public DateTime Created { get; set; }
        public List<Prediction> Predictions { get; set; }
    }

    public class Prediction
    {
        public double Probability { get; set; }
        public string TagId { get; set; }
        public string TagName { get; set; }
    }
}
