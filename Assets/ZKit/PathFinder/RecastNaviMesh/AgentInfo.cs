namespace ZKit.PathFinder
{
    public class AgentInfo
    {
        private float _agentRadius; // 플레이어 반지름
        private float _agentHeight; // 플레이어 높이
        private float _maxSlope;    // 오를수 있는 경사도
        private float _maxClimb;    // 오를수 있는 높이

        public float AgentHeight { get { return _agentHeight; } set { _agentHeight = value; } }
        public float AgentRadius { get { return _agentRadius; } set { _agentRadius = value; } }
        public float MaxSlope { get { return _maxSlope; } set { _maxSlope = value; } }
        public float MaxClimb { get { return _maxClimb; } set { _maxClimb = value; } }
    }
}