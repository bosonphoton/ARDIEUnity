#!/usr/bin/env python

import speech_recognition as sr
from sound_play.libsoundplay import SoundClient
import time

speaker = SoundClient()
ear = sr.Recognizer()

def hear(observation):

	if observation == "areaa":
		print("Red/A")
		speaker.say("Did you mean Red?")
		time.sleep(2)
		with sr.Microphone() as source:
			ear.adjust_for_ambient_noise(source)
			print("Retriving Y/N")
			audio = ear.listen(source,timeout=4)
		try:
			text = ear.recognize_google(audio)
			print(text)
			return text
		except:
			print("I did not hear")


		
	elif observation == "areab":
		print("Green/B")
		speaker.say("Did you mean Green?")
		time.sleep(2)
		with sr.Microphone() as source:
			ear.adjust_for_ambient_noise(source)
			print("Retriving Y/N")
			audio = ear.listen(source,timeout=4)
		try:
			text = ear.recognize_google(audio)
			print(text)
			return text
		except:
			print("I did not hear")




	else:
		print("Blue/C")
		speaker.say("Did you mean Blue?")
		time.sleep(2)
		with sr.Microphone() as source:
			ear.adjust_for_ambient_noise(source)
			print("Retriving Y/N")
			audio = ear.listen(source,timeout=4)
		try:
			text = ear.recognize_google(audio)
			print(text)
			return text
		except:
			print("I did not hear")














