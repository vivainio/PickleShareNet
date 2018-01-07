from __future__ import print_function

import os,shutil

def c(s):
    print(">",s)
    err = os.system(s)
    assert not err

shutil.rmtree("PickleShareNet/bin")
shutil.rmtree("PickleShareNet/obj")

os.chdir("Test")
c("dotnet test")

os.chdir("../PickleShareNet")
c("dotnet pack -c Release")