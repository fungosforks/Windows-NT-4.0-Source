//  Microsoft is required by the Educational Testing Service and the
//  College Entrance Examination Board to include the statement below in
//  Microsoft products that use the AP Computer Science C++ Classes.  The
//  inclusion of this statement does not imply endorsement by Microsoft.
//  Microsoft disclaims any and all warranties, express or implied,
//  arising out of the following statement and shall not be liable for any
//  direct or indirect damages related to the statement.

//  Inclusion of the C++ classes defined for use in the Advanced Placement
//  Computer Science courses does not constitute endorsement of the other
//  material in this [textbook, CD-ROM, web site, manual, etc.] by the
//  College Board, Educational Testing Service, or the AP Computer Science
//  Development Committee.  The versions of the C++ classes defined for
//  use in the AP Computer Science courses included in this product were
//  accurate as of 11/30/2000. Revisions to the classes may have been made
//  since that time. See the College Board web site at
//  http://www.collegeboard.com/ for the latest version of these classes.

// *******************************************************************
//  Last Revised: 8/18/98
//                abort() changed to exit(1)
//                comments updated
//
//  September 1, 1997 -- APCS stack class  IMPLEMENTATION
//
//  stack implemented using the APCS vector class
// *******************************************************************

#include "apstack.h"
#include <stdlib.h>

const int SDEFAULT_SIZE = 10;        // default initial stack size

template <class itemType>
apstack<itemType>::apstack( )
    : myTop(-1),
      myElements(SDEFAULT_SIZE)
     
// postcondition: the stack is empty     
{
    
}

template <class itemType>
apstack<itemType>::apstack(const apstack<itemType> & s)
   : myTop(s.myTop),     
     myElements(s.myElements)

// postcondition: stack is a copy of s     
{
    
}
     
template <class itemType>
apstack<itemType>::~apstack()
// postcondition: stack is destroyed
{
    // vector destructor frees memory
}

template <class itemType>
const apstack<itemType> &
apstack<itemType>::operator = (const apstack<itemType> & rhs)
// postcondition: normal assignment via copying has been performed
{
    if (this != &rhs)
    {
        myTop = rhs.myTop;
        myElements = rhs.myElements;
    }
    return *this;
}

template <class itemType>
bool
apstack<itemType>::isEmpty() const
// postcondition: returns true if stack is empty, false otherwise
{
    return myTop == -1;
}

template <class itemType>
int
apstack<itemType>::length() const
// postcondition: returns # of elements currently in stack
{
    return myTop+1;
}

template <class itemType>
void
apstack<itemType>::push(const itemType & item)
// precondition: stack is [e1, e2...en] with  n >= 0
// postcondition: stack is [e1, e2, ... en, item]     
{
    if( myTop + 1 >= myElements.length() )   // grow vector if necessary
    {
        myElements.resize(myElements.length() * 2);
    }
    myTop++;                           // new top most element
    myElements[myTop ] = item;
}

template <class itemType>
void
apstack<itemType>::pop()
// precondition: stack is [e1,e2,...en] with n >= 1
// postcondition: stack is [e1,e2,...e(n-1)]
{
    if (isEmpty())
    {
        cerr << "error, popping an empty stack" << endl;
        exit(1);
    }
    myTop--;
}

template <class itemType>
void
apstack<itemType>::pop(itemType & item)
// precondition: stack is [e1,e2,...en] with n >= 1
// postcondition: stack is [e1,e2,...e(n-1)] and item == en     
{
    if (isEmpty())
    {
        cerr << "error, popping an empty stack" << endl;
        exit(1);
    }
    item = myElements[myTop];
    myTop--;
}

template <class itemType>
const itemType &
apstack<itemType>::top() const
// precondition: stack is [e1, e2, ... en] with n >= 1
// postcondition: returns en
{
    if (isEmpty())
    {
        cerr << "error, popping an empty stack" << endl;
        exit(1);
    }    
    return myElements[myTop];
}

template <class itemType>
void
apstack<itemType>::makeEmpty()
// postcondition:  stack is empty     
{
    myTop = -1;    
}
