from __future__ import print_function

import os,shutil

def c(s):
    print(">",s)
    os.system(s)

shutil.rmtree("PickleShareNet/bin")
shutil.rmtree("PickleShareNet/obj")

os.chdir("PickleShareNet")
c("dotnet pack -c Release")