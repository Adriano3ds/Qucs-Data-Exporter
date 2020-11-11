using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Qucs_Data_Exporter
{
    class QucsSimData
    {
        private string name;
        private string dependecy;
        private int lenght;
        private double[] values;

        public QucsSimData(string dependecy, string name, int lenght, double[] values)
        {
            this.dependecy = dependecy;
            this.name = name;
            this.lenght = lenght;
            this.values = values;
        }

        public QucsSimData(GroupCollection groupCollection)
        {
            dependecy = groupCollection[1].Value;
            name = groupCollection[2].Value;
            //lenght = int.Parse(groupCollection[3].Value);
            List<double> data = new List<double>();
            foreach(string s in groupCollection[4].Value.Split('\n'))
            {
                try
                {
                    data.Add(Double.Parse(s, new CultureInfo("en-US")));
                }catch(Exception e)
                {

                }
            }
            values = data.ToArray();
        }

        public string Name
        {
            get => name;
        }

        public string Dependency
        {
            get => dependecy;
        }

        public int Lenght
        {
            get => lenght;
        }

        public double[] Values
        {
            get => values;
        }
    }
}
