using System.Diagnostics.CodeAnalysis;
using TV;
using Xunit;

namespace TestsTV
{
    public class TestsTV
    {
        [Fact]
        public void Just_created_TV_is_turned_off_and_current_channel_is_zero()
        {
            // arrange
            TVSet tv = new();

            // act
            var (isOn, currentChannel) = tv.GetTVInfo();

            // assert
            Assert.False(isOn);
            Assert.Equal(0, currentChannel);
        }

        [Fact]
        public void Turning_on_when_TV_turned_off()
        {
            // arrange
            TVSet tv = new();

            // act
            tv.TurnTVOn();

            // assert
            Assert.True(tv.GetTVInfo().isOn);
            Assert.Equal(1, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Turning_on_when_TV_turned_on()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();

            // act
            tv.TurnTVOn();

            // assert
            Assert.True(tv.GetTVInfo().isOn);
            Assert.Equal(1, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Turned_off_TV_always_shows_channel_zero()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();
            tv.SetTVChannel(10);

            // act
            tv.TurnTVOff();

            // assert
            Assert.False(tv.GetTVInfo().isOn);
            Assert.Equal(0, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Turning_off_when_TV_turned_on()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();

            // act
            tv.TurnTVOff();

            // assert
            Assert.False(tv.GetTVInfo().isOn);
            Assert.Equal(0, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Turning_off_when_TV_turned_off()
        {
            // arrange
            TVSet tv = new();

            // act
            tv.TurnTVOff();

            // assert
            Assert.False(tv.GetTVInfo().isOn);
            Assert.Equal(0, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_channel_on_disabled_TV()
        {
            // arrange
            TVSet tv = new();

            // act
            tv.SetTVChannel(10);

            // assert
            Assert.False(tv.GetTVInfo().isOn);
            Assert.Equal(0, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_channel_on_turned_on_TV()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();

            // act
            tv.SetTVChannel(5);

            // assert
            Assert.True(tv.GetTVInfo().isOn);
            Assert.Equal(5, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_channel_with_minimum_number()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();

            // act
            tv.SetTVChannel(1);

            // assert
            Assert.True(tv.GetTVInfo().isOn);
            Assert.Equal(1, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_channel_with_number_zero()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();

            // act
            tv.SetTVChannel(0);

            // assert
            Assert.True(tv.GetTVInfo().isOn);
            Assert.Equal(1, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_channel_with_number_less_one()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();

            // act
            tv.SetTVChannel(-1);

            // assert
            Assert.True(tv.GetTVInfo().isOn);
            Assert.Equal(1, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_channel_with_max_number()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();

            // act
            tv.SetTVChannel(100);

            // assert
            Assert.True(tv.GetTVInfo().isOn);
            Assert.Equal(100, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_channel_with_number_more_than_100()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();

            // act
            tv.SetTVChannel(101);

            // assert
            Assert.True(tv.GetTVInfo().isOn);
            Assert.Equal(1, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_previous_channel_after_first_turning_on_TV()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();

            // act
            tv.SetTVPreviousChannel();

            // assert
            Assert.Equal(1, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_previous_channel_after_select_another_channel()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();
            tv.SetTVChannel(5);
            tv.SetTVChannel(7);

            // act
            tv.SetTVPreviousChannel();

            // assert
            Assert.Equal(5, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_previous_channel_after_turning_off_and_turning_on()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();
            tv.SetTVChannel(5);
            tv.SetTVChannel(7);
            tv.TurnTVOff();
            tv.TurnTVOn();

            // act
            tv.SetTVPreviousChannel();

            // assert
            Assert.Equal(5, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_previous_channel_after_two_times_selecting_same_channel()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();
            tv.SetTVChannel(5);
            tv.SetTVChannel(7);
            tv.SetTVChannel(7);

            // act
            tv.SetTVPreviousChannel();

            // assert
            Assert.Equal(5, tv.GetTVInfo().currentChannel);
        }
        [Fact]
        public void Select_previous_channel_two_times()
        {
            // arrange
            TVSet tv = new();
            tv.TurnTVOn();
            tv.SetTVChannel(5);
            tv.SetTVChannel(7);

            // act
            tv.SetTVPreviousChannel();
            tv.SetTVPreviousChannel();

            // assert
            Assert.Equal(7, tv.GetTVInfo().currentChannel);
        }
    }
}
