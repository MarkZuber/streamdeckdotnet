From: https://github.com/abcminiuser/python-elgato-streamdeck

## Raspberry Pi Installation:

The following script has been verified working on a Raspberry Pi (Model 2 B)
running a stock Debian Stretch image, to install all the required dependencies
needed by this project:

```
# Ensure system is up to date, upgrade all out of date packages
sudo apt update && sudo apt dist-upgrade -y

# Install the pip Python package manager
sudo apt install -y python3-pip

# Install system packages needed for the Python hidapi package installation
sudo apt install -y libudev-dev libusb-1.0-0-dev

# Install dependencies
pip3 install hidapi

# Add udev rule to allow all users non-root access to Elgato StreamDeck devices:
sudo tee /etc/udev/rules.d/10-streamdeck.rules << EOF
	SUBSYSTEMS=="usb", ATTRS{idVendor}=="0fd9", ATTRS{idProduct}=="0060", GROUP="users"
	SUBSYSTEMS=="usb", ATTRS{idVendor}=="0fd9", ATTRS{idProduct}=="0063", GROUP="users"
	EOF

# Install the latest version of the StreamDeck library via pip
pip3 install streamdeck

# Alternatively, install git and check out the repository
#sudo apt install -y git
#git clone https://github.com/abcminiuser/python-elgato-streamdeck.git
```

Note that after adding the `udev` rule, a restart will be required in order for
it to take effect and allow access to the StreamDeck device without requiring
root privileges.
