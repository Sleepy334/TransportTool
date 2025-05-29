using static TransportInfo;

namespace PublicTransportInfo
{
    public class StopInfo
    {
        // Currently viewed line info
        public TransportType m_transportType = TransportType.Bus;
        public int m_currentLineId = 0;
        public ushort m_currentStopId = 0;
        public int m_stopNumber = 0;

        // ----------------------------------------------------------------------------------------
        public StopInfo()
        {
        }

        public StopInfo(StopInfo oSecond)
        {
            m_transportType = oSecond.m_transportType;
            m_currentLineId = oSecond.m_currentLineId;
            m_currentStopId = oSecond.m_currentStopId;
            m_stopNumber = oSecond.m_stopNumber;
        }

        public bool Equals(StopInfo oSecond)
        {
            return m_transportType == oSecond.m_transportType && 
                   m_currentLineId == oSecond.m_currentLineId &&
                   m_currentStopId == oSecond.m_currentStopId;
        }

        public void Set(TransportType type, int lineId, ushort stopId, string lineName, int stopNumber)
        {
            m_transportType = type;
            m_currentLineId = lineId;
            m_currentStopId = stopId;
            m_stopNumber = stopNumber;
        }

        public void Reset()
        {
            m_transportType = TransportType.Bus;
            m_currentLineId = 0;
            m_currentStopId = 0;
            m_stopNumber = 0;
        }

        public string Describe()
        {
            return $"TransportType: {m_transportType} LineId: {m_currentLineId} StopId: {m_currentStopId} StopNumber: {m_stopNumber}";
        }
    }
}