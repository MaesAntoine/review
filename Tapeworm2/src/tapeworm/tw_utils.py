import inspect
import tw_classes
import os
import subprocess
import sys
import shlex

def double_backslash(input_string):
    """
    Doubles all backslashes in the input string.
    
    Args:
    input_string (str): The input string to process.
    
    Returns:
    str: The string with all backslashes doubled.
    """
    return input_string.replace("\\", "\\\\")

def is_tw_class(input_object):
    """Check if TsW is a class instance from tw_classes.py.

    Args:
        TsW (object): Object to be checked.

    Raises:
        ValueError: TsW is not a Tapeworm setting.

    """
    classes = [cls for _, cls in inspect.getmembers(tw_classes, inspect.isclass)]
    if any(isinstance(input_object, cls) for cls in classes) is False:
        raise ValueError(
            "TwS is not a Tapeworm setting."
        )  # maybe not optimal to raise an error here


def get_both_extensions(input_object):
    """Get the file extension of the source file and the target file.

    Args:
        O (object): Object to be checked.

    Returns:
        tuple: Tuple of file extensions.
    """

    # check if input_object is a class instance from tw_classes.py
    is_tw_class(input_object)

    source_extension = input_object.source_path.split(".")[-1]
    target_extension = input_object.target_path.split(".")[-1]
    return source_extension, target_extension


def rename_files(filepath, force=False):

    input_path = filepath[1].strip("'\"")

    if not os.path.isdir(input_path):
        raise ValueError("Input directory not found: {0}".format(input_path))

    files = [f for f in os.listdir(input_path) if os.path.isfile(os.path.join(input_path, f))]
    num_files = len(files)
    padding = len(str(num_files))

    # Check if files are already renamed
    if not force and all(f.startswith("file_") and f[5].isdigit() for f in files):
        return "file_%0{0}d".format(padding)

    # prepare new filenames
    for i, filename in enumerate(sorted(files), start=1):
        old_path = os.path.join(input_path, filename)
        new_filename = "file_{0:0{1}d}{2}".format(i, padding, os.path.splitext(filename)[1])
        new_path = os.path.join(input_path, new_filename)
        
        # Rename files
        if old_path != new_path:
            try:
                os.rename(old_path, new_path)
            except OSError as e:
                print("Error: Failed to rename file {0} to {1}".format(old_path, new_path))

    return "file_%0{0}d".format(padding)


def get_updated_command(input_object, pattern):
    # go through all attributes of the object and build a string using the key and value
    cmd_parts = ['"C:\\ffmpeg-6.0-essentials_build\\bin\\ffmpeg.exe"']
    for key, value in input_object.__dict__.items():
        if not isinstance(value, list):
            continue

        if key == "input":
            cmd_parts.extend([value[0], '"' + value[1] + pattern + '.png"'])
            continue

        if key == "output":
            continue
        
        cmd_parts.extend(value)
    
    cmd_parts.extend(['"' + input_object.output[1] + '"'])

    return ' '.join(cmd_parts)

# def call_ffmpeg(command):
#     # call the ffmpeg command
#     print("Debug: Full FFmpeg command:")
#     print(command)

#     subprocess.run(command, shell=True)

def call_ffmpeg(cmd):
    """Invokes a shell process by command using the subprocess module.
    Args:
      cmd (str): A shell command starting with the tool name or path,
        followed by some arguments.
    Returns:
      The standard output [0] and standard error [1] of the process.
    """
    args = shlex.split(cmd)
    # Run the command using subprocess
    process = subprocess.Popen(args, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    stdout, stderr = process.communicate()
    return stdout, stderr