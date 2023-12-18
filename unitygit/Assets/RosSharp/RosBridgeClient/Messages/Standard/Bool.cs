/* 
 * This message is auto generated by ROS#. Please DO NOT modify.
 * Note:
 * - Comments from the original code will be written in their own line 
 * - Variable sized arrays will be initialized to array of size 0 
 * Please report any issues at 
 * <https://github.com/siemens/ros-sharp> 
 */

using Newtonsoft.Json;

namespace RosSharp.RosBridgeClient.MessageTypes.Std
{
    public class Bool : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "std_msgs/Bool";

        public bool data;

        public Bool()
        {
            this.data = false;
        }

        public Bool(bool data)
        {
            this.data = data;
        }
    }
}
