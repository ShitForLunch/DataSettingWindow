using System;

namespace DataSettingWindowProject
{
    public class VectorND
    {
        public Double[] Data;

        private Int32 Dimension = 0;
        private Int32 Indexer = 0;

        public VectorND(Int32 dim)
        {
            Dimension = dim;
            Data = new Double[Dimension];
        }

        public void Append(Double num)
        {
            Data[Indexer++] = num;
        }

        public static Double EuclideanDistance(VectorND v1, VectorND v2)
        {
            if (v1.Dimension != v2.Dimension)
                return Double.NaN;

            Int32 dim = v1.Dimension;
            Double ss = 0;     // sum of square
            Double result;
            for (Int32 i = 0; i < dim; i++)
                ss += Math.Pow(v1.Data[i] - v2.Data[i], 2);
            result = Math.Sqrt(ss);

            return result;
        }
    }
}
