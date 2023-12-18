#!/usr/bin/env python

import rospy
import actionlib
from move_base_msgs.msg import MoveBaseAction, MoveBaseGoal, MoveBaseFeedback
from actionlib_msgs.msg import *
from geometry_msgs.msg import Pose, Point, PoseWithCovarianceStamped, Quaternion, PoseStamped
from nav_msgs.srv import GetPlan
from nav_msgs.msg import Odometry
from std_msgs.msg import String
from move_base_msgs.msg import MoveBaseAction, MoveBaseGoal
from sound_play.libsoundplay import SoundClient
import time
from datetime import datetime


speaker = SoundClient()
last_color = ""
last_calltime = datetime.strptime("00:00:00","%H:%M:%S")


def callback(data):
	colors = open("colors.txt", "a")
	global last_calltime, last_color
	cube = data.data
	t = time.localtime()
	tf = time.strftime("%H:%M:%S",t)
	current_time = datetime.strptime(tf, "%H:%M:%S")
	delta_time = current_time - last_calltime

	if abs(delta_time.total_seconds()) >= 3:
		if "Red" in cube:
			if last_color != "Red":
				print("Red/AA")
				#speaker.say("Did you mean Red?")
				last_color = "Red"
				colors.write("\n")
				colors.write("\nareaa")
				colors.close()

		elif "Green" in cube:
			if last_color != "Green":
				print("Green/Bb")
				#speaker.say("Did you mean Green?")
				last_color = "Green"
				colors.write("\n")
				colors.write("\nareab")
				colors.close()
				

		else:
			if last_color != "Blue":
				print("Blue/CC")
				#speaker.say("Did you mean Blue?")
				last_color = "Blue"
				colors.write("\n")
				colors.write("\nareac")
				colors.close()


		t = time.localtime()
		tf = time.strftime("%H:%M:%S",t)
		last_calltime = datetime.strptime(tf, "%H:%M:%S")


	return last_color


if __name__ == '__main__':

	try:
		rospy.init_node('speak_object')
		rospy.Subscriber ('/gaze_vector',String, callback)
		rospy.spin()

	except rospy.ROSInterruptException:
		print "finished!"

