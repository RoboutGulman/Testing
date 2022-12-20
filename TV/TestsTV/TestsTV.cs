using System.Diagnostics.CodeAnalysis;
using TV;
using Xunit;

namespace TestsTV
{

public class TestsTV
{
    private readonly TVSet _tv;
    public TestsTV()
    {
        _tv = new();
    }
    [Fact]
    public void Get_info_of_just_created_TV()
    {
        // arrange

        // act
        var (isOn, currentChannel) = _tv.GetTVInfo();

        // assert
        Assert.False(isOn);
        Assert.Equal(0, currentChannel);
    }

    [Fact]
    public void Turning_on_when_TV_turned_off()
    {
        // arrange

        // act
        _tv.TurnTVOn();

        // assert
        Assert.True(_tv.GetTVInfo().isOn);
        Assert.Equal(1, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Turning_on_when_TV_turned_on()
    {
        // arrange
        _tv.TurnTVOn();

        // act
        _tv.TurnTVOn();

        // assert
        Assert.True(_tv.GetTVInfo().isOn);
        Assert.Equal(1, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Turned_off_TV_always_shows_channel_zero()
    {
        // arrange
        _tv.TurnTVOn();
        _tv.SetTVChannel(10);

        // act
        _tv.TurnTVOff();

        // assert
        Assert.False(_tv.GetTVInfo().isOn);
        Assert.Equal(0, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Turning_off_when_TV_turned_on()
    {
        // arrange
        _tv.TurnTVOn();

        // act
        _tv.TurnTVOff();

        // assert
        Assert.False(_tv.GetTVInfo().isOn);
        Assert.Equal(0, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Turning_off_when_TV_turned_off()
    {
        // arrange

        // act
        _tv.TurnTVOff();

        // assert
        Assert.False(_tv.GetTVInfo().isOn);
        Assert.Equal(0, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_channel_on_turned_off_TV()
    {
        // arrange

        // act
        _tv.SetTVChannel(10);

        // assert
        Assert.False(_tv.GetTVInfo().isOn);
        Assert.Equal(0, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_channel_on_turned_on_TV()
    {
        // arrange
        _tv.TurnTVOn();

        // act
        _tv.SetTVChannel(5);

        // assert
        Assert.True(_tv.GetTVInfo().isOn);
        Assert.Equal(5, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_channel_with_minimum_number()
    {
        // arrange
        _tv.TurnTVOn();

        // act
        _tv.SetTVChannel(1);

        // assert
        Assert.True(_tv.GetTVInfo().isOn);
        Assert.Equal(1, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_channel_with_number_zero()
    {
        // arrange
        _tv.TurnTVOn();

        // act
        _tv.SetTVChannel(0);

        // assert
        Assert.True(_tv.GetTVInfo().isOn);
        Assert.Equal(1, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_channel_with_negative_number()
    {
        // arrange
        _tv.TurnTVOn();

        // act
        _tv.SetTVChannel(-1);

        // assert
        Assert.True(_tv.GetTVInfo().isOn);
        Assert.Equal(1, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_channel_with_max_number()
    {
        // arrange
        _tv.TurnTVOn();

        // act
        _tv.SetTVChannel(100);

        // assert
        Assert.True(_tv.GetTVInfo().isOn);
        Assert.Equal(100, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_channel_with_number_more_than_100()
    {
        // arrange
        _tv.TurnTVOn();

        // act
        _tv.SetTVChannel(101);

        // assert
        Assert.True(_tv.GetTVInfo().isOn);
        Assert.Equal(1, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_previous_channel_after_first_turning_on_TV()
    {
        // arrange
        _tv.TurnTVOn();

        // act
        _tv.SetTVPreviousChannel();

        // assert
        Assert.Equal(1, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_previous_channel_after_select_another_channel()
    {
        // arrange
        _tv.TurnTVOn();
        _tv.SetTVChannel(5);
        _tv.SetTVChannel(7);

        // act
        _tv.SetTVPreviousChannel();

        // assert
        Assert.Equal(5, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_previous_channel_after_select_one_channel()
    {
        // arrange
        _tv.TurnTVOn();
        _tv.SetTVChannel(7);

        // act
        _tv.SetTVPreviousChannel();

        // assert
        Assert.Equal(1, _tv.GetTVInfo().currentChannel);
    }

    [Fact]
    public void Select_previous_channel_after_turning_off_and_turning_on()
    {
        // arrange
        _tv.TurnTVOn();
        _tv.SetTVChannel(5);
        _tv.SetTVChannel(7);
        _tv.TurnTVOff();
        _tv.TurnTVOn();

        // act
        _tv.SetTVPreviousChannel();

        // assert
        Assert.Equal(5, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_previous_channel_after_two_times_selecting_same_channel()
    {
        // arrange
        _tv.TurnTVOn();
        _tv.SetTVChannel(5);
        _tv.SetTVChannel(7);
        _tv.SetTVChannel(7);

        // act
        _tv.SetTVPreviousChannel();

        // assert
        Assert.Equal(5, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_previous_channel_two_times()
    {
        // arrange
        _tv.TurnTVOn();
        _tv.SetTVChannel(5);
        _tv.SetTVChannel(7);

        // act
        _tv.SetTVPreviousChannel();
        _tv.SetTVPreviousChannel();

        // assert
        Assert.Equal(7, _tv.GetTVInfo().currentChannel);
    }
    [Fact]
    public void Select_previous_channel_on_turned_off_TV()
    {
        // arrange
        _tv.TurnTVOn();
        _tv.SetTVChannel(5);
        _tv.SetTVChannel(7);
        _tv.TurnTVOff();

        // act
        _tv.SetTVPreviousChannel();
        _tv.TurnTVOn();

        // assert
        Assert.Equal(7, _tv.GetTVInfo().currentChannel);
    }
}
}
