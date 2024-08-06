import subprocess
import os
import sys
import re
import glob
import subprocess

class BaseVideoProcessor:
    def get_data(self):
        # Flatten the list of lists into a single list
        return [item for sublist in self.__dict__.values() if isinstance(sublist, list) for item in sublist]
    
    def __repr__(self):
        # Join all items in get_data() into a string, representing an ffmpeg command
        return ' '.join(str(item) for item in self.get_data())

class Input(BaseVideoProcessor):
    def __init__(self, previous_object=None, source_path=""):
        if previous_object is not None and hasattr(previous_object, '__dict__'):
            self.__dict__.update(previous_object.__dict__)
        self.input = ["-i", source_path]
        
class Output(BaseVideoProcessor):
    def __init__(self, previous_object, target_path):
        if hasattr(previous_object, '__dict__'):
            self.__dict__.update(previous_object.__dict__)
        
            # Add default settings for mp4
        if target_path.endswith(".mp4"):
            self.mp4_codec = ["-c:v", "libx264"]
            self.mp4_pixel_format = ["-pix_fmt", "yuv420p"]
            self.mp4_balance = ["-crf", "23"]
            self.mp4_scale = ["-vf", "scale=768:768"]
        
        self.output = ["-o", target_path]

class Framerate(BaseVideoProcessor):
    def __init__(self, previous_object, framerate):
        if hasattr(previous_object, '__dict__'):
            self.__dict__.update(previous_object.__dict__)
        self.framerate = ["-framerate", str(int(framerate))]


class StartFrame(BaseVideoProcessor):
    def __init__(self, previous_object, start_frame):
        if hasattr(previous_object, '__dict__'):
            self.__dict__.update(previous_object.__dict__)
        self.start_frame = ["-start_number", str(int(start_frame))]


