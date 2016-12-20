using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace howto_hexagonal_grid
{
    public class RandomTable<T>
    {
        protected readonly List<RandomElement<T>> elementsBase;
        private Random rand;
        private double sum;

        public RandomTable(List<T> equallyDistributedElements){
            this.elementsBase = equallyDistributedElements.Select(e => new RandomElement<T>(){ Prob=1.0, Element=e}).ToList();
            Init();
        }

        public RandomTable(List<RandomElement<T>> elements){
            this.elementsBase = elements;
            Init();
        }

        public static RandomTable<T> AddTables(List<RandomTable<T>> tables){
            var newElements = new List<RandomElement<T>>();
            foreach(var table in tables){
                newElements.AddRange(table.elementsBase);
            }
            return new RandomTable<T>(newElements);
        }

        private void Init()
        {
            this.rand = new Random();
            this.sum = elementsBase.Select(x => x.Prob).Sum();
        }

        public T Get()
        {
          double roll = rand.NextDouble() * sum;
          double runningTotal = 0.0;
          for (int i = 0; i < elementsBase.Count; i++)
          {
              runningTotal = runningTotal + elementsBase[i].Prob;
              if (roll < runningTotal)
              {
                  return elementsBase[i].Element;
              }
          }
          throw new InvalidOperationException("Bug!");
        }

    }

    public class RandomElement<T>
    {
        public double Prob { get; set; }
        public T Element { get; set; }
    }

}
