from tw_data import SUPPORTED_IMAGE_FORMATS, SUPPORTED_VIDEO_FORMATS
from tw_utils import is_tw_class


# Don't look a this class yet. It's not used.
class Task:
    def __init__(self):
        self.task = None
        self.required_settings = None
        self.forbidden_settings = None
        self.source_check = None
        self.target_check = None


# Don't look a this class yet. It's not used.
class ImageToGif(Task):
    def __init__(self, input_object):
        super().__init__()
        self.task = "image_to_gif"
        self.required_settings = [
            input_object.get_input(),
            input_object.get_output(),
        ]
        self.forbidden_settings = [
            input_object.get_bitrate(),
        ]
        self.source_check = input_object.get_input() in SUPPORTED_IMAGE_FORMATS
        self.target_check = input_object.get_output() == "gif"


def is_task_image_to_gif(input_object):
    """
    Typical tapeworm function.
    Args:
        input_object (object): Tapeworm object to be checked.

    Returns:
        bool: True if the task is image to gif.
    """

    required_settings = [
        input_object.get_input(),
        input_object.get_output(),
    ]
    forbidden_settings = [
        input_object.get_bitrate(),
    ]
    # check if required and forbidden settings are ok
    required_ok = all(required_settings)
    forbidden_ok = not any(forbidden_settings)

    # check if source and target are ok
    source_check = input_object.get_input() in SUPPORTED_IMAGE_FORMATS
    target_check = input_object.get_output() == "gif"

    if all([required_ok, forbidden_ok, source_check, target_check]):
        return True
    return False


def is_task_image_to_video(input_object):
    """
    Typical tapeworm function.

    Args:
        input_object (object): Tapeworm object to be checked.

    Returns:
        bool: True if the task is image to video.
    """

    required_settings = [
        input_object.get_input(),
        input_object.get_output(),
    ]
    forbidden_settings = [None]
    # check if required and forbidden settings are ok
    required_ok = all(required_settings)
    forbidden_ok = not any(forbidden_settings)

    # check if source and target are ok
    source_check = input_object.get_input() in SUPPORTED_IMAGE_FORMATS
    target_check = input_object.get_output() in SUPPORTED_VIDEO_FORMATS

    if all([required_ok, forbidden_ok, source_check, target_check]):
        return True
    return False


def is_task_gif_to_image(input_object):
    """
    Typical tapeworm function.

    Args:
        input_object (object): Tapeworm object to be checked.

    Returns:
        bool: True if the task is gif to image.
    """

    required_settings = [
        input_object.get_input(),
        input_object.get_output(),
    ]
    forbidden_settings = [
        input_object.get_bitrate(),
    ]
    # check if required and forbidden settings are ok
    required_ok = all(required_settings)
    forbidden_ok = not any(forbidden_settings)

    # check if source and target are ok
    source_check = input_object.get_input() == "gif"
    target_check = input_object.get_output() in SUPPORTED_IMAGE_FORMATS

    if all([required_ok, forbidden_ok, source_check, target_check]):
        return True
    return False


def is_task_video_to_image(input_object):
    """
    Typical tapeworm function.

    Args:
        input_object (object): Tapeworm object to be checked.

    Returns:
        bool: True if the task is video to image.
    """

    required_settings = [
        input_object.get_input(),
        input_object.get_output(),
    ]
    forbidden_settings = [
        input_object.get_bitrate(),
    ]

    # check if required and forbidden settings are ok
    required_ok = all(required_settings)
    forbidden_ok = not any(forbidden_settings)

    # check if source and target are ok
    source_check = input_object.get_input() in SUPPORTED_VIDEO_FORMATS
    target_check = input_object.get_output() in SUPPORTED_IMAGE_FORMATS

    if all([required_ok, forbidden_ok, source_check, target_check]):
        return True
    return False


def is_task_gif_to_video(input_object):
    """
    Typical tapeworm function.

    Args:
        input_object (object): Tapeworm object to be checked.

    Returns:
        bool: True if the task is gif to video.
    """

    required_settings = [
        input_object.get_input(),
        input_object.get_output(),
    ]
    forbidden_settings = [None]

    # check if required and forbidden settings are ok
    required_ok = all(required_settings)
    forbidden_ok = not any(forbidden_settings)

    # check if source and target are ok
    source_check = input_object.get_input() == "gif"
    target_check = input_object.get_output() in SUPPORTED_VIDEO_FORMATS

    if all([required_ok, forbidden_ok, source_check, target_check]):
        return True
    return False


def is_task_video_to_gif(input_object):
    """
    Typical tapeworm function.

    Args:
        input_object (object): Tapeworm object to be checked.

    Returns:
        bool: True if the task is video to gif.
    """

    required_settings = [
        input_object.get_input(),
        input_object.get_output(),
    ]
    forbidden_settings = [
        input_object.get_bitrate(),
    ]

    # check if required and forbidden settings are ok
    required_ok = all(required_settings)
    forbidden_ok = not any(forbidden_settings)

    # check if source and target are ok
    source_check = input_object.get_input() in SUPPORTED_VIDEO_FORMATS
    target_check = input_object.get_output() == "gif"

    if all([required_ok, forbidden_ok, source_check, target_check]):
        return True
    return False


def is_task_image_to_image(input_object):
    """
    Typical tapeworm function.

    Args:
        input_object (object): Tapeworm object to be checked.

    Returns:
        bool: True if the task is image to image.
    """

    required_settings = [
        input_object.get_input(),
        input_object.get_output(),
    ]
    forbidden_settings = [
        input_object.get_bitrate(),
    ]

    # check if required and forbidden settings are ok
    required_ok = all(required_settings)
    forbidden_ok = not any(forbidden_settings)

    # check if source and target are ok
    source_check = input_object.get_input() in SUPPORTED_IMAGE_FORMATS
    target_check = input_object.get_output() in SUPPORTED_IMAGE_FORMATS

    if all([required_ok, forbidden_ok, source_check, target_check]):
        return True
    return False


def is_task_video_to_video(input_object):
    """
    Typical tapeworm function.

    Args:
        input_object (object): Tapeworm object to be checked.

    Returns:
        bool: True if the task is video to video.
    """

    required_settings = [
        input_object.get_input(),
        input_object.get_output(),
    ]
    forbidden_settings = [None]

    # check if required and forbidden settings are ok
    required_ok = all(required_settings)
    forbidden_ok = not any(forbidden_settings)

    # check if source and target are ok
    source_check = input_object.get_input() in SUPPORTED_VIDEO_FORMATS
    target_check = input_object.get_output() in SUPPORTED_VIDEO_FORMATS

    if all([required_ok, forbidden_ok, source_check, target_check]):
        return True
    return False


def is_task_add_audio_to_video(input_object):
    # this one would get two -i files... a video and an audio file
    return False


def is_task_extract_audio_from_video(input_object):
    return False


function_list = [
    is_task_image_to_gif,
    is_task_image_to_video,
    is_task_gif_to_image,
    is_task_video_to_image,
    is_task_gif_to_video,
    is_task_video_to_gif,
    is_task_image_to_image,
    is_task_video_to_video,
    is_task_add_audio_to_video,
    is_task_extract_audio_from_video,
]


def detector(input_object):
    """Detect the task of the input object.

    Args:
        input_object (object): Object to be checked.

    Raises:
        ValueError: TsW is not a Tapeworm setting.

    Returns:
        function: Function that performs the task.
    """
    is_tw_class(input_object)
    for function in function_list:
        if function(input_object):
            return function.__name__
    raise ValueError("Task could not be detected.")
