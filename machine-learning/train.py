import os
import torch
import pandas as pd
import torch.nn as nn
from torch.optim import Adam
from skimage.io import imread
from sklearn.model_selection import train_test_split

from utils import *

## Define a function to train the model.
def trainModel(dataPath):

    ## Defining the path to the data directory.
    imgNames = os.listdir(dataPath)
    
    ## Reading in the dataframe.
    dataFrame = pd.read_csv(dataPath + '/Annotation.csv')

    ## Convert the labels to categorical data format.
    dataFrame['Label'] = dataFrame['Label'].astype('category')
    dataFrame['Label'] = dataFrame['Label'].cat.codes
    
    ## Preprocessing function for reading the images.
    grayscale = True
    height = 256
    width = 256
    
    ## Prepare the training data.
    imgList = []
    labList = []

    for index, rowData in dataFrame.iterrows():
        imgID, labID = rowData['Name'], rowData['Label']
        try:
            img = imread(dataPath + '/' + imgID, as_gray = True)
            img = preprocess(img, height, width)
            imgList.append(img)
            labList.append(labID)
        except ValueError:
            continue
    
    ## Training Data.
    X = np.array(imgList)
    y = np.asarray(labList)
    
    ## Fix the dimensionality of the data.
    X = X.reshape(X.shape[0], 1, X.shape[1], X.shape[2])
    
    ## Split the data into training/testing splits.
    XTrain, XTest, yTrain, yTest = train_test_split(X, y, test_size = 0.1)
    print('Dataset Preparation Done!')
    
    ## Instantiate the model.
    model = Net()

    ## Define the optimizer.
    optimizer = Adam(model.parameters(), lr = 3e-4, weight_decay = 1e-5)

    ## Define the loss function.
    criterion = nn.CrossEntropyLoss()
    
    ## Obtain the batch-indices for training and testing data.
    batchSize = 32
    numTrainBatches = int(len(yTrain)/batchSize)
    numTestBatches = int(len(yTest)/batchSize)

    trainIndices = np.array_split(range(len(yTrain)), numTrainBatches)
    testIndices = np.array_split(range(len(yTest)), numTestBatches)
    
    numEpochs = 20

    ## List for storing the loss values and accuracy values.
    lossVals = []
    testAccuracyVals = []

    for epoch in range(numEpochs):

        ## Put the model in train mode.
        model.train()

        ## Keep track of the total loss.
        totLoss = 0

        for i in range(numTrainBatches):

            ## Obtain the current batch.
            xTrue, yTrue = XTrain[trainIndices[i]], yTrain[trainIndices[i]]
            xTrue, yTrue = torch.from_numpy(xTrue), torch.from_numpy(yTrue)

            ## Clear the gradient.
            optimizer.zero_grad()

            ## Feed-Forward through the model.
            opBatch = model(xTrue)
            _, yPred = torch.max(opBatch, 1)

            ## Computing the loss.
            loss = criterion(opBatch, yTrue)
            totLoss += loss.item()

            ## Backprop.
            loss.backward()
            optimizer.step()

        lossVals.append(totLoss)
        print('Epoch : ', epoch)
        print('Loss : ', totLoss)

        ## Variable for computing the accuracy.
        totalTest = 0
        correctTest = 0

        for i in range(numTestBatches):

            ## Obtain the current batch.
            xTrue, yTrue = XTest[testIndices[i]], yTest[testIndices[i]]
            xTrue, yTrue = torch.from_numpy(XTest), torch.from_numpy(yTest)

            ## Feed-Forward through the model.
            opBatch = model(xTrue)
            _, yPred = torch.max(opBatch, 1)

            ## Statistics for totalling.
            totalTest += yTrue.shape[0]
            correctTest += torch.sum(yPred == yTrue)

        testAccuracy = correctTest.item()/totalTest
        testAccuracyVals.append(testAccuracy)
        print('Testing Accuracy is : ', testAccuracy)
        print(' ')
        
    ## Save the models.
    torch.save(model.state_dict(), './trainedModel.pth')
    
trainModel('./Data')