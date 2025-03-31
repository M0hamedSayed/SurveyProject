using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application.Interfaces
{
    public interface IChoiceSurveyDto
    {
        public Guid? Id { get; }
        public  string TextAr { get; set; }
        public string TextEn { get; set; }
    }

    public interface IEvaluateChoiceSurveyDto
    {
        public Guid? Id { get; }
        public string TextAr { get; set; }
        public string TextEn { get; set; }
        public string Emotion { get; set; }
    }
}
