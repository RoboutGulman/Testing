namespace TV;

public class TVSet
{
    private const int _minChannel = 1;
    private const int _maxChannel = 100;
    private bool _isOn;
    private int _currentChannel, _previousChannel;

    public TVSet()
    {
        _isOn = false;
        _currentChannel = 0;
        _previousChannel = 0;
    }

    public (bool isOn, int currentChannel) GetTVInfo()
    {
        if (!_isOn)
        {
            return (_isOn, 0);
        }
        return (_isOn, _currentChannel);
    }

    public void TurnTVOn()
    {
        _isOn = true;

        if (_currentChannel == 0)
        {
            _currentChannel = 1;
        }
    }

    public void TurnTVOff()
    {
        _isOn = false;
    }

    public bool SetTVChannel(int channel)
    {
        if (_isOn && channel >= _minChannel && channel <= _maxChannel && channel != _currentChannel)
        {
            _previousChannel = _currentChannel;
            _currentChannel = channel;
            return true;
        }
        return false;
    }

    public void SetTVPreviousChannel()
    {
        if (_isOn && _previousChannel != 0)
        {
            (_previousChannel, _currentChannel) = (_currentChannel, _previousChannel);
        }
    }
}
