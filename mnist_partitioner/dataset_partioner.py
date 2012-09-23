import gzip
import os
import cPickle
import numpy
import math

REDUCED_DATASET_PATH = '../reduced_mnist.pkl'
MNIST_DATASET_PATH = '../deep_learning_tutorial/data/mnist.pkl.gz'

# How long should training and fine-tuning the DBN take per population member?
REQUESTED_DBN_TIME_MIN = 15

# Empirical data from running it on my i5 mac mini.
PRE_TRAIN_DBN_TIME_MIN = 1099.02
FINE_TUNE_DBN_TIME_MIN = 438.34

TOTAL_DBN_TIME_MIN = PRE_TRAIN_DBN_TIME_MIN + FINE_TUNE_DBN_TIME_MIN

# This is how much we should reduce our dataset by. We don't need a fancy formula
# Because we're just decreasing the workload here and not changing the fundamental
# algorithm
REDUCTION_FACTOR =  TOTAL_DBN_TIME_MIN / REQUESTED_DBN_TIME_MIN


def download_mnist_dataset_if_missing():
    data_dir, data_file = os.path.split(MNIST_DATASET_PATH)
    if (not os.path.isfile(MNIST_DATASET_PATH)) and data_file == 'mnist.pkl.gz':
        import urllib
        origin = 'http://www.iro.umontreal.ca/~lisa/deep/data/mnist/mnist.pkl.gz'
        print 'Downloading data from %s' % origin
        urllib.urlretrieve(origin, MNIST_DATASET_PATH)

    print "Loading MNIST Data"
    print ""

def reduce_dataset(name, dataset_pair):

    # The classifications are in the first index of the tuple
    classifications = dataset_pair[1]

    # First Pass on List: Create a bucket for each digit and sum them
    digit_count_buckets = {}
    reduced_digit_count_buckets = {}

    for current_digit in range(10):
        digit_count_buckets[current_digit] = 0

    for current_digit in list(classifications):
        digit_count_buckets[current_digit] += 1

    print "Initial " + name + " is:", digit_count_buckets

    for current_digit in range(10):
        digit_count_buckets[current_digit] /= REDUCTION_FACTOR
        digit_count_buckets[current_digit] = int(digit_count_buckets[current_digit])

    reduced_digit_count_buckets = digit_count_buckets.copy()

    # The images are in the zero-th index of the tuple
    images = dataset_pair[0]

    # How the heck do you get python to infer the type of a variable without
    # initializing it?
    reduced_images = numpy.zeros(shape=(1, 784))
    reduced_classifications = []

    for index in range(len(classifications)):
        current_digit = classifications[index]

        if reduced_digit_count_buckets[current_digit] > 0:
            # Copy one of the images from our numpy array of the MNIST images to our new array numpy array
            reduced_images = numpy.vstack( [ reduced_images , images[index] ] )
            reduced_classifications.append(current_digit)
            reduced_digit_count_buckets[current_digit] -= 1

    reduced_images = numpy.delete(reduced_images, 0, 0)

    # Count up our new list
    for current_digit in list(reduced_classifications):
        reduced_digit_count_buckets[current_digit] += 1

    # Sanity check that we did the copy correctly
    assert reduced_digit_count_buckets == digit_count_buckets
    print "Reduced " + name + " to:", reduced_digit_count_buckets
    print ""

    return (reduced_classifications, reduced_images)



def load_mnist_dataset():
    download_mnist_dataset_if_missing()

    print "Reduction Factor is:", REDUCTION_FACTOR
    print ""

    # Load the dataset
    f = gzip.open(MNIST_DATASET_PATH, 'rb')
    train_set, valid_set, test_set = cPickle.load(f)
    f.close()

    # Reduce them all by a factor (tuning params at the top of the file)
    reduced_datasets = (reduce_dataset("Training Set",   train_set),
                        reduce_dataset("Validation Set", valid_set),
                        reduce_dataset("Testing Set",    test_set))

    train_set_count, valid_set_count, test_set_count = (len(reduced_datasets[0][0]),
                                                        len(reduced_datasets[1][0]),
                                                        len(reduced_datasets[2][0]))

    cPickle.dump( reduced_datasets, open( REDUCED_DATASET_PATH, 'wb' ) )
    print "Created reduced_mnist.pkl"

    print "Checking consistency of the pickle."

    # Load the dataset
    train_set, valid_set, test_set = cPickle.load(open(REDUCED_DATASET_PATH, 'rb'))
    f.close()

    assert train_set_count == len(train_set[0])
    assert valid_set_count == len(valid_set[0])
    assert test_set_count == len(test_set[0])

    print "The pickle is good."

    #TODO: How to make a 'reduced_mnist_pkl.tar.gz?

if __name__ == '__main__':
    load_mnist_dataset()

