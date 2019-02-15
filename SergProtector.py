# -*- coding: utf-8 -*-
"""
Created on Mon Nov  5 20:23:33 2018

@author: serge
"""
import numpy as np
import time
import os
import csv
import matplotlib.pyplot as plt

def readfile(filepath,isLive,doplots,explicit=True):
    with open(filepath,'r') as csvfile:
        firstread = csv.reader(csvfile, delimiter=',')
#        headers = list(firstread)[0]
        print("didit")
        dat = np.array(list(firstread)[1:-1])
    csvfile.close()
    length = len(dat)
    if explicit:
        print("File successfully read.\nThere are "+str(length) + " lines of data.")
    if isLive:
        init = 1
        if doplots:
            limit = 2000
            x = np.linspace(1,limit,limit)
            ydat = np.zeros(limit)
            plt.ion()
            fig = plt.figure()
            ax = fig.add_subplot(111)
            line1, = ax.plot(x, ydat, 'r-')
        running = True
        while running:
            with open(filepath,'r') as csvfile:
                reread = csv.reader(csvfile, delimiter=',')
                data = np.array(list(reread))
            if len(data) > length:
                diff = len(data)-length
                print(str(diff)+" new lines")
                length = len(data)
                init = 1
                time.sleep(0.5)
                """
                updating plot
                """
                if doplots:
                    print(data)
                    dats = np.array(list(data[:-1]))
                    dats = dats[-int(diff):,3]
                    ydat = ydat[diff:]
                    ydat = np.append(ydat,dats)
                    line1.set_ydata(ydat)
                    ax.relim()
                    ax.autoscale_view()
                    fig.canvas.draw()
                    fig.canvas.flush_events()
            elif init < 10:
                init += 1
                time.sleep(1)
                print("no new lines... retrying x" + str(init))
            else:
                running=False
                csvfile.close()
    
#    return(dat,headers)
    return(dat)


relativepath = '\\EEGLogger\\bin\\Debug\\EEGLogger.csv'
cwd = os.getcwd()
fpath = cwd + relativepath

#foobar = str(input("Live? (Y/N) \nENTER: ")).upper()
foobar = "nope"
plotting = "nah"
#plotting = str(input("Plotting? (Y/N) \nENTER: ")).upper()
if foobar =="Y":
    isLive = True
else:
    isLive = False
if plotting=="Y":
    doplots = True
else:
    doplots = False
alldat = np.array(readfile(fpath,isLive,doplots))
#headers = alldat[0]
dat = np.array(alldat[1:]).astype(float)
print(dat)
###############################################################################
###############################################################################
############################  PLOTTING  STUFF  ################################
###############################################################################
###############################################################################
if isLive==False:
    length = len(dat)
    #types = len(dat[0])
    headers = dat[0]
    fullXaxis = np.linspace(1,length,length)
    plt.plot(fullXaxis,dat[:,3])

    limit = 2000
    x = np.linspace(1,limit,limit)
    ydat = np.zeros(limit)
    plt.ion()
    fig = plt.figure()
    ax = fig.add_subplot(111)
    line1, = ax.plot(x, ydat, 'r-')
    
    # Returns a tuple of line objects, thus the comma
#    for chunk in fullXaxis:
    numchunks = 1000
    chunksize = int(length/numchunks)
    datcol3 = dat[:,3]
    for chunk in range(numchunks):
        start = chunk*chunksize
        end = (chunk+1)*chunksize-1
        ydat = ydat[chunksize-1:]
        ydat = np.append(ydat,datcol3[start:end])
        #    print((phase,ydat))
        line1.set_ydata(ydat)
        ax.relim()
        ax.autoscale_view()
        fig.canvas.draw()
        fig.canvas.flush_events()
    
    fullXaxis = np.linspace(1,length,length)

    plt.plot(fullXaxis,datcol3)
    plt.title(headers[3])
    plt.show()
    #fig.tight_layout() #multiple
    #plt.show()
    
