import numpy, scipy
from scipy import interpolate

kernelIn = numpy.array([
    [0,1,1,0],
    [1,2,2,1],
    [1,2,2,1],
    [0,1,1,0] ])

inKSize = len(kernelIn)
outKSize = 10

kernelOut = numpy.zeros((outKSize),numpy.uint8)

x = numpy.array(range(inKSize))
y = numpy.array(range(inKSize))

z = kernelIn

xx = numpy.linspace(x.min(),x.max(),outKSize)
yy = numpy.linspace(y.min(),y.max(),outKSize)

newKernel = interpolate.RectBivariateSpline(x,y,z, kx=3,ky=3)

kernelOut = newKernel(xx,yy)

print numpy.floor(kernelOut)
