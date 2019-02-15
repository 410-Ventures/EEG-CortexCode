# -*- coding: utf-8 -*-
"""
Created on Mon Nov  5 20:23:33 2018

@author: serge
"""
"""
TESTING HOW WELL I CAN RUN SUB-PROCESSES FROM A PYTHON SCRIPT
"""

import os
import subprocess

relativepath = '\\EEGLogger\\bin\\Debug\\EEGLogger.exe'
cwd = os.getcwd()
fpath = cwd + relativepath

#os.system(fpath)
proc = subprocess.Popen(fpath)
proc.communicate()
#proc = subprocess.Popen(fpath)
#try:
#    outs, errs = proc.communicate(timeout=3)
#except TimeoutExpired:
#    proc.kill()
#    outs, errs = proc.communicate()