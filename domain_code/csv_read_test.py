import numpy
import theano
import timeit
from numpy import genfromtxt

t = timeit.Timer('numpy.genfromtxt("../rng.csv", delimiter=";", dtype=theano.config.floatX)', 'import numpy, theano')
print t.timeit(1)

# 784 x 1000
csv_record = genfromtxt('../rng.csv', delimiter=';', dtype=theano.config.floatX)
# Should be close to 0.500
print numpy.average(csv_record)

n_in = 784
n_out = 1000
high = numpy.sqrt(6. / (n_in + n_out))
translate_factor = high
scale_factor = 1 / (2*high)
# This is what we need to divide by to get an average which is
print scale_factor
csv_record /= scale_factor
# This should be the same as high
print numpy.average(csv_record)
print high

csv_record -= translate_factor

# This should be roughly zero
print numpy.average(csv_record)

print csv_record


'''
            W_values = numpy.asarray(rng.uniform(
                    low=-numpy.sqrt(6. / (n_in + n_out)),
                    high=numpy.sqrt(6. / (n_in + n_out)),
                    size=(n_in, n_out)), dtype=theano.config.floatX)
                    '''