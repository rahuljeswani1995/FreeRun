import torch
from skimage.io import imread

## Preprocessing function for reading the images.
grayscale = True
height = 256
width = 256


## Define a function for producing the output.
def makePrediction(modelPath, imagePath):
    
    ## Read the image and pre-process it.
    X = imread(imagePath, as_gray = True)
    X = preprocess(X)
    X = np.asarray(X)

    ## Fix the dimensionality of the data.
    X = X.reshape(1, 1, X.shape[0], X.shape[1])
    X = np.array(X)

    ## Load the model for inference.
    loadedModel = Net()
    loadedModel.load_state_dict(torch.load('./trainedModel.pth'))

    ## Test the model for inference.
    loadedModel.eval()
    
    ## Feed-Forward through the model.
    opBatch = model(X)
    _, yPred = torch.max(opBatch, 1)
    return yPred