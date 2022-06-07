#Project 13 - Burglar Detector With Photo Capture
#latest code updates available at: https://github.com/RuiSantosdotme/RaspberryPiProject
#project updates at: https://nostarch.com/RaspberryPiProject

#import the necessary packages
from gpiozero import Button, MotionSensor
from picamera import PiCamera
from time import sleep
from signal import pause
import requests
import os


#create objects that refer to a button,
#a motion sensor and the PiCamera
button = Button(2)
pir = MotionSensor(4)
camera = PiCamera()

#start the camera
camera.start_preview()

url = 'http://functionsip:7071/api/analyzeimage'

#stop the camera when the pushbutton is pressed
def stop_camera():
    print('Terminating...')
    camera.stop_preview()
    #exit the program
    exit()

#take photo when motion is detected
def take_photo():   
    fileName = '/home/pi/greeter.jpg'
    camera.capture(fileName)
    headers = {
    'conten-type': "image/jpeg"
    }
    print('A photo has been taken')
    jsonheaderup={'Content-Type': 'application/octet-stream'}
    with open(fileName, 'rb') as file:
        requests.post(url, data=file, verify=False, headers=jsonheaderup, timeout=None)
    sleep(1)
    os.remove(fileName)

#assign a function that runs when the button is pressed
button.when_pressed = stop_camera
#assign a function that runs when motion is detected
pir.when_motion = take_photo

pause()