# GrassVR - Grasshopper VR integration tool

## Grass VR interaction for Form Finding and Structural Analysis proof of concept.

The files in the repository are the summary of the development work executed for my thesis work "Exploring the Potential of Cross Reality in Architectural, Civil Engineering and Construction - From general sector usage to the educational potential of a Virtual Reality design tool" at ULB and VUB universities.
As such the joint ownership of the work by the user and the universities exist. The thesis work has been published as an accessible and open to access process.
The final thesis work is kept as it was in the first alpha release. While possible future release could be executed at the moment no clear roadmap for the project are presented.

Before starting the presentation of the directories, it is needed to specify tha this repository and the present Unity project are not at a release state. In fact GrassVR is a WIP and my thesis work is the starting point to generate the core functionality and first test environment.

So, this file are not intended at for public usage, but mainly as a presentation element to permit the access to the developed code to the thesis jury. As this marged Unity project steal need some configuration for the translation from the test/debug system internal to unity and the realese of the Alpha version.

## Repository structure

The repository is divided between:
1) [Unity Project files](GRASSVR_UnityProject/)
2) [Rhino and Grasshopper files](Rhino-Grasshopper_files/)\
    Contain the relevant Grasshopper files as the examples set-ups and the Grasshopper scripts
3) [Other test file](Other_files/)\
   contain the C# console test files and some Kangaroo related test files that are not directly connected to GrassVR but have been used during analysis of the Grasshopper's components</p>
For the Unity project all the required data is present: the StamVR asset is kept in the project at newer version of the asset could break sone of the developed interaction and input binding elements.

The main folder containing the developed data is the [Asset folder](GRASSVR_UnityProject/Assets/)

The Unity folder contain the main developed elements and some scenes that are full and ready for the manual connection to Grasshopper via the custom C# components. However, the usage of this project is no considered reliable with as the generation of an Alpha release is on its way after the submission and discussion of my work.

The Alpha release will be presented with usage examples as they are presented and discussed in my thesis manuscript. At the moment my focus is on the conclusion of my work. Only after that the conversion from the development state to the Alpha will be realised.

## Road Map
[Future development steps](GRASSVR_UnityProject/Roadmap.md)
