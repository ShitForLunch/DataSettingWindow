using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using CsvHelper;
using MathNet.Numerics.Statistics;

namespace DataSettingWindowProject
{
    public class DataContainer
    {
        public List<List<String>> data = new List<List<String>>();
        public List<VectorND> vectors = new List<VectorND>();
        public List<VectorND> vectors_norm = new List<VectorND>();
        public List<String> numHeaders = new List<String>();
        public List<String> catHeaders = new List<String>();
        public List<String> uids = new List<String>();

        public String filePath;
        public Int32 row;
        public Int32 column;
        public Int32 count;                 // the number of items
        public Int32 dimension;             // the number of numerical attributes

        public Boolean hasIndex = false;
        public Int32 headerIndex = 0;       // (Row) header index
        public Int32 uidIndex;              // (Column) unique id index
        public List<Int32> numIndex;
        public List<Int32> catIndex;

        public Boolean isNormalized = false;
        public Boolean noHeader = false;

        public List<VectorND> GetVectors()
        {
            if (isNormalized == false)
                return vectors;
            else
                return vectors_norm;
        }

        public void SetFilePath(String path)
        {
            filePath = path;
        }

        public void SetIndex(Int32 uid, List<Int32> num, List<Int32> cat)
        {
            uidIndex = uid;
            numIndex = num;
            catIndex = cat;
            hasIndex = true;
        }

        public void SetHeaders(Boolean h, List<String> headers)
        {
            noHeader = h;
            // Numerical Header
            for (Int32 i = 0; i < numIndex.Count; i++)
            {
                Int32 col = numIndex[i];
                numHeaders.Add(headers[col]);
            }
            dimension = numIndex.Count;

            // Categorical Header
            for (Int32 i = 0; i < catIndex.Count; i++)
            {
                Int32 col = catIndex[i];
                catHeaders.Add(headers[col]);
            }
        }

        public Boolean ParseFileToString()
        {
            if (filePath == null)
                return false;

            if (filePath.Contains(".csv"))
            {
                try
                {
                    List<String> result = new List<String>();
                    String value;
                    using (TextReader fileReader = File.OpenText(filePath))
                    {
                        var csv = new CsvReader(fileReader);
                        csv.Configuration.HasHeaderRecord = false;
                        while (csv.Read())
                        {
                            for (int i = 0; csv.TryGetField<String>(i, out value); i++)
                            {
                                result.Add(value);
                            }
                            data.Add(result);
                        }
                    }

                    /*
                    List<IList<String>> lines = CSVParser.FromFile(filePath).ToList();
                    row = lines.Count;
                    column = lines[0].Count;
                    for (Int32 i = 0; i < lines.Count; i++)
                    {
                        List<String> rowlist = lines[i].ToList();
                        data.Add(rowlist);
                    }
                    */
                }
                catch (System.IO.IOException e)
                {
                    MessageBox.Show("File could not be opened.\nCheck whether your file is currently in use.");
                    return false;
                }
            }
            return true;
        }

        public void ParseStringToNumber()
        {
            if (filePath == null || hasIndex == false)
                return;

            // Vector
            for (Int32 i = 0; i < row; i++)
            {
                if (noHeader == false && i == headerIndex)
                    continue;

                VectorND vector = new VectorND(dimension);
                for (Int32 j = 0; j < numIndex.Count; j++)
                {
                    Int32 col = numIndex[j];
                    Double converted;
                    Double.TryParse(data[i][col], out converted);
                    vector.Append(converted);
                }
                vectors.Add(vector);
            }
            count = vectors.Count;

            // Vector_Norm
            for (Int32 i = 0; i < count; i++)
            {
                VectorND vector = new VectorND(dimension);
                vectors_norm.Add(vector);
            }

            // Unique ID
            if (uidIndex == -1)
            {
                // Pseudo Unique ID를 만든다.
                Int32 iter = 1;
                String template = "Item ";
                for (Int32 i = 0; i < count; i++, iter++)
                    uids.Add(template + iter.ToString());
            }
            else
            {
                for (Int32 i = 0; i < row; i++)
                {
                    if (i == headerIndex)
                        continue;

                    uids.Add(data[i][uidIndex]);
                }
            }
        }

        public void Normalization(Boolean colbycol)
        {
            // 현재는 Standardize 만을 지원한다.
            if (colbycol == true)
            {
                for (Int32 i = 0; i < dimension; i++)
                {
                    Double[] target = new Double[count];
                    for (Int32 j = 0; j < count; j++)
                        target[j] = vectors[j].Data[i];
                    Double mean = target.Average();
                    Double stdev = target.PopulationStandardDeviation();
                    for (Int32 j = 0; j < count; j++)
                        vectors_norm[j].Data[i] = (target[j] - mean) / stdev;
                }
            }
            else
            {
                for (Int32 i = 0; i < count; i++)
                {
                    Double mean = vectors[i].Data.Average();
                    Double stdev = vectors[i].Data.PopulationStandardDeviation();
                    for (Int32 j = 0; j < dimension; j++)
                        vectors_norm[i].Data[j] = (vectors[i].Data[j] - mean) / stdev;
                }
            }
            isNormalized = true;
        }

        private void PrintVectors(Boolean norm)
        {
            List<VectorND> source;
            if (norm == false)
                source = vectors;
            else
                source = vectors_norm;

            Console.WriteLine("============================================");
            Console.WriteLine("Print ParsedData Vectors : ");
            for (Int32 i = 0; i < count; i++)
            {
                for (Int32 j = 0; j < dimension; j++)
                {
                    Console.Write(source[i].Data[j] + " ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("============================================");
        }

    }
}
