using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.DataTypes
{
    public class CPDataSequenceAbs
    {
        public SensorType sensorType;
        public List<CPVectorAbs> sequenceAbs;
        public long startTime;

        public CPDataSequenceAbs(SensorType sensorType, long startTime)
        {
            this.sensorType = sensorType;
            this.startTime = startTime;

            sequenceAbs = new List<CPVectorAbs>();
        }

        public void addVector(CPVectorAbs vector)
        {
            sequenceAbs.Add(vector);
        }

        public List<CPVectorAbs> getRange(long startTime, long endTime)
        {
            int startIndex = getClosestVectorIndex(startTime);
            int endIndex = getClosestVectorIndex(endTime);

            if (sequenceAbs[startIndex].absoluteTime > startTime)
                startIndex--;
            if (sequenceAbs[endIndex].absoluteTime < endTime)
                endIndex++;

            return sequenceAbs.GetRange(startIndex, endIndex - startIndex);
        }

        public CPVectorAbs getClosestVector(long time)
        {
            return sequenceAbs[getClosestVectorIndex(time)];
        }

        public int getClosestVectorIndex(long time)
        {
            CPVectorAbs centerVector = sequenceAbs[0];
            int elementsLeft = sequenceAbs.Count;
            int startIndex = 0;
            int endIndex = sequenceAbs.Count;

            while (elementsLeft > 4)
            {
                int centerIndex = startIndex + elementsLeft / 2;
                centerVector = sequenceAbs[centerIndex];

                if (time > centerVector.absoluteTime)
                    startIndex = centerIndex;
                else endIndex = centerIndex;

                elementsLeft = endIndex - startIndex;

                if (centerVector.absoluteTime == time)
                    return centerIndex;
            }

            long minDelta = Math.Abs(time - sequenceAbs[startIndex].absoluteTime);
            int minIndex = startIndex;

            for (int i = startIndex + 1; i < endIndex; i++)
            {
                long delta = Math.Abs(time - sequenceAbs[i].absoluteTime);
                if (delta < minDelta)
                {
                    minDelta = delta;
                    minIndex = i;
                }
            }

            return minIndex;
        }
    }

    public class CPDataSequence
    {
        public SensorType sensorType;
        public List<Sequence> sequences;
        public long startTime;        
        public double maxDeviationPerc;

        private Sequence currSequence;

        public CPDataSequence(SensorType sensorType, long startTime, double maxDeviationPerc = 0.4)
        {
            this.sensorType = sensorType;
            this.startTime = startTime;
            this.maxDeviationPerc = maxDeviationPerc;

            sequences = new List<Sequence>();
            addSeq();
        }

        public void addVector(CPVector vector, long absoluteTime)
        {
            bool normalDeviation = currSequence.addVector(vector, absoluteTime);
            if (!normalDeviation)
            {
                addSeq();
                addVector(vector, absoluteTime);
            }
        }
        
        public CPVector? getVector(long absoluteTime)
        {
            int seqIndex = getSeqIndex(absoluteTime);
            if (seqIndex == -1)
                return null;

            return sequences[seqIndex].getVector(absoluteTime);
        }

        private int getSeqIndex(long absoluteTime)
        {
            //non-binary search :(
            if (absoluteTime < sequences[0].startTime || absoluteTime > sequences.Last().endTime)
                return -1;
            
            for (int i = 0; i < sequences.Count; i++)
                if (sequences[i].endTime < absoluteTime)
                    continue;
                else if (sequences[i].startTime <= absoluteTime)
                    return i;
                else
                    return -1;

            return -1;
        }

        private void addSeq()
        {
            Sequence firstSeq = new Sequence(maxDeviationPerc);
            sequences.Add(firstSeq);
            currSequence = firstSeq;
        }
    }

    public class Sequence
    {
        public long startTime;
        public long endTime;
        public long dinamicInterval;
        public double maxDeviationPerc;
        public List<CPVector> vectors;
        public Func<CPVector, long, bool> addVector;        

        public Sequence(double maxDeviationPerc)
        {
            this.maxDeviationPerc = maxDeviationPerc;
            vectors = new List<CPVector>();
            addVector = addInitialVector;
        }

        public CPVector? getVector(long absoluteTime)
        {
            int index = getVectorIndex(absoluteTime);

            if (index == -1)
                return null;

            return vectors[index];
        }

        public List<CPVector> getRange(long startTime, long endTime)
        {
            return vectors.GetRange(getVectorIndex(startTime), getVectorIndex(endTime));
        }

        public int getVectorIndex(long absoluteTime)
        {            
            if (absoluteTime < startTime || absoluteTime > endTime)
                return -1;

            CPVector centerVector = vectors[0];
            int elementsLeft = vectors.Count;
            int startIndex = 0;
            int endIndex = vectors.Count;

            while (elementsLeft > 4)
            {
                int centerIndex = startIndex + elementsLeft / 2;
                long centerAbsTime = getAbsoluteTime(centerIndex);
                centerVector = vectors[centerIndex];

                if (absoluteTime > centerAbsTime)
                    startIndex = centerIndex;
                else endIndex = centerIndex;

                elementsLeft = endIndex - startIndex;

                if (centerAbsTime == absoluteTime)
                    return centerIndex;
            }

            long minDelta = Math.Abs(absoluteTime - getAbsoluteTime(startIndex));
            int minIndex = startIndex;

            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                long delta = Math.Abs(absoluteTime - getAbsoluteTime(i));
                long a = getAbsoluteTime(6);
                long b = getAbsoluteTime(7);
                if (delta < minDelta)
                {
                    minDelta = delta;
                    minIndex = i;
                }
            }

            return minIndex;
        }

        public long getAbsoluteTime(int position)
        {
            return startTime + dinamicInterval * position;
        }        

        private bool addInitialVector(CPVector vector, long absoluteTime)
        {
            startTime = absoluteTime;
            vectors.Add(vector);
            endTime = absoluteTime;

            addVector = addSecondVector;

            return true;
        }

        private bool addSecondVector(CPVector vector, long absoluteTime)
        {            
            dinamicInterval = absoluteTime - startTime;
            vectors.Add(vector);
            endTime = absoluteTime;

            addVector = addOneMoreVector;

            return true;
        }

        private bool addOneMoreVector(CPVector vector, long absoluteTime)
        {
            long interval = absoluteTime - endTime;
            long intervalDelta = Math.Abs(interval - dinamicInterval);

            if (intervalDelta > dinamicInterval * maxDeviationPerc)
                return false;

            dinamicInterval = (dinamicInterval * vectors.Count + interval) / (vectors.Count + 1);
            vectors.Add(vector);

            endTime = absoluteTime;
            return true;
        }
    }
}
