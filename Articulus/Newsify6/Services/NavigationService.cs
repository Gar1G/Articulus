﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Newsify6.Services
{
    public class NavigationService
    {
        /// <summary>
        /// This holds the instance to the Only NavigationService in this app.
        /// </summary>
        public static NavigationService Instance { get; protected set; }

        /// <summary>
        /// This will hold the reference to the frame that is to be manipulated.
        /// </summary>
        private Frame frame;

        /// <summary>
        /// The Stack of pages to enable Stack based Navigation.
        /// </summary>
        public Stack<Type> PageStack { get; protected set; }

        #region CTOR
        /// <summary>
        /// The default constructor to instantiate this class with reference to a frame
        /// </summary>
        /// <param name="frame">The referenced frame</param>
        public NavigationService(ref Frame frame)
        {
            //Check is the instance doesnt already exist.
            if (Instance != null)
            {
                //if there is an instance in the app already present then simply throw an error.
                throw new Exception("Only one navigation service can exist in a App.");
            }
            //setting the instance to the static instance field.
            Instance = this;
            //setting the frame reference.
            this.frame = frame;
            //intialising the stack.
            this.PageStack = new Stack<Type>();


            //Hooking up the events for BackRequest both for Big Windows and for Phone.

            SystemNavigationManager.GetForCurrentView().BackRequested += NavigationService_BackRequested;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed; ;
            }

        }
        #endregion

        #region Navigation Methods
        public void NavigateTo(Type pageType, object parameter)
        {
            frame.Navigate(pageType, parameter);
        }

        public void NavigateBack()
        {
            if (frame.CanGoBack)
                frame.GoBack();
            PageStack.Pop();
            UpdateBackButtonVisibility();
        }

        public void NavigateToHome()
        {
            while (frame.CanGoBack)
                frame.GoBack();
        }
        #endregion

        #region BackButtonVisibilty Region
        void UpdateBackButtonVisibility()
        {
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = frame.CanGoBack ?
            //     AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }
        #endregion

        #region Event Methods for windows and phone
        private void NavigationService_BackRequested(object sender, BackRequestedEventArgs e)
        {
            this.NavigateBack();
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            this.NavigateBack();
        }
        #endregion

    }
}
