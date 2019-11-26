# **track-modification**
---

## Addin Revit to track the database modification change. 
`The original solution is of Jeremy Tamik
[Track Change](https://github.com/jeremytammik/TrackChanges)

This solution is created to adapt the demand of my company.

<a name="My idea"></a>

# :heavy_check_mark: My idea

  - Using the button for starting and ending the button: change the text and image when clicked
  - By defaut, addin will run automaticaly when revit start and stop when revit close
  - The elements created, modified will stocked the date changed in the 2 shared parameter
  - One parameter indicate date of today, one parameter will use for the status of changing
  - Create the addin manager to show, color, select the changed element by jour

<a name="installation"></a>

# Todo
- [x] Ribbon Revit!
  - [x] Solution Track Change
    - [x] Add succesfully the created date, modified date to element
    - [x] Run automatique when the document opened
    - [x] Change the button text when running or not running
    - [ ] How change the button image when running or not running
    - [ ] Check if shared parameter exist (CreateDate, ModifiedDate), if not create them
    - [ ] Run the add with the frequency by 1 minute/5 minutes/10 minutes...
- [ ] Solution Dynamic model update
    - [ ] Update automatic to element
    - [ ] Check and create parameter as the solution Track change
    - [ ] Compare 2 solutions to adapt the addin manager future
    - [ ] Create CustomPushButton
- [ ] Addin modification manager
    - [ ] Create GUI With WPF
    - [ ] Get list of element changes, and take them iamges or id
    - [ ] Possibility of select them by addin
    - [ ] Change color automatique with the changed elements
    - [ ] How to get the list modification xml, txt...?
    - [ ] To think...
    








```So let's go!``` 

**Good luck for me.**




<a name="license"></a>

# :scroll: License

MIT © [Tien Duy NGUYEN](https://https://github.com/TienDuyNGUYEN)

:baby_chick:

---
