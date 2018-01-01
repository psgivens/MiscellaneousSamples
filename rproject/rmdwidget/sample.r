
return;

####
## Getting to here! ####
#                  ##
######
#setwd("C:/Repos/GH/MiscellaneousSamples/rproject/rmdwidget")
#install.packages("htmlwidgets")
#install.packages("devtools")
#devtools::create("psgwidget")
#setwd("psgwidget")
#htmlwidgets::scaffoldWidget("psgwidget")



##########
#### This is the meat of this file ###
#                            ########
                           #######

# Set the directory and get the values
setwd("C:/Repos/GH/MiscellaneousSamples/rproject/rmdwidget/psgwidget")
defects <- read.csv("defects.csv")

# Reinstalling the widget and run it. 
devtools::install()
library(psgwidget)
psgwidget(message="Hello to the world", data=defects)






# Play with selection
defects[which(defects$severity=="High"),]
defects$severity

# Check out jsonlite. Supposedly used to convert for htmlwidets
library(jsonlite)
jsonlite::toJSON(defects[which(defects$severity=="High"),])


