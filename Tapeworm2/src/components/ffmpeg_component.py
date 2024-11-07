import tapeworm2_setup
tapeworm2_setup.setup_tapeworm_path()
import sys
from tw_utils import rename_files, call_ffmpeg, get_updated_command




if R:
    pattern = rename_files(TwS.input)
    full_command = get_updated_command(TwS, pattern)
    print (full_command)

    call_ffmpeg(full_command)