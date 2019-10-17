﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CefSharp;
using Newtonsoft.Json;
using Rhino;
//using CefSharp.Wpf;
using SpeckleUiBase;

namespace SpeckleRhino.UIBindings
{
  internal partial class RhinoUiBindings : SpeckleUIBindings
  {
    public List<dynamic> Clients;

    public bool SelectionExpired = true;

    public RhinoUiBindings( IWebBrowser myBrowser )
    {
      Browser = myBrowser;
      Clients = new List<dynamic>();

      // Selection Events
      RhinoDoc.SelectObjects += ( sender, e ) => { if( !Browser.IsBrowserInitialized ) return; SelectionExpired = true; };
      RhinoDoc.DeselectObjects += ( sender, e ) => { if( !Browser.IsBrowserInitialized ) return; SelectionExpired = true; };
      RhinoDoc.DeselectAllObjects += ( sender, e ) => { if( !Browser.IsBrowserInitialized ) return; SelectionExpired = true; };
      RhinoApp.Idle += RhinoApp_Idle;

      RhinoDoc.BeginSaveDocument += RhinoDoc_BeginSaveDocument;
    }

    private void RhinoDoc_BeginSaveDocument( object sender, DocumentSaveEventArgs e )
    {
      SaveClients();
    }

    private void RhinoApp_Idle( object sender, EventArgs e )
    {
      if( !Browser.IsBrowserInitialized ) return;
      if( SelectionExpired )
      {
        SelectionExpired = false;
        var selectedObjectsCount = RhinoDoc.ActiveDoc.Objects.GetSelectedObjects( false, false ).ToList().Count;
        NotifyUi( "update-selection-count", JsonConvert.SerializeObject( new
        {
          selectedObjectsCount
        } ) );
      }
    }

    public void GetOldFileClients()
    {
      //TODO: migrate old clients to new clients. somehow.
      string[ ] receiverKeys = RhinoDoc.ActiveDoc.Strings.GetEntryNames( "speckle-client-receivers" );
      string[ ] senderKeys = RhinoDoc.ActiveDoc.Strings.GetEntryNames( "speckle-client-senders" );
    }

    public override void ShowAccountsPopup()
    {
      Rhino.RhinoApp.InvokeOnUiThread( new Action( () =>
      {
        var signInWindow = new SpecklePopup.SignInWindow();
        var helper = new System.Windows.Interop.WindowInteropHelper( signInWindow );
        helper.Owner = Rhino.RhinoApp.MainWindowHandle();

        signInWindow.ShowDialog();
        DispatchStoreActionUi( "getAccounts" );
      } ) );
    }

    public override void RemoveClient( string args )
    {
      var client = JsonConvert.DeserializeObject<dynamic>( args );
      var index = Clients.FindIndex( cl => cl.clientId == client.clientId );
      if( index < 0 ) return;
      Clients.RemoveAt( index );
      SaveClients();
    }

    public void SaveClients()
    {
      var doc = RhinoDoc.ActiveDoc;
      doc.Strings.SetString( "speckle", JsonConvert.SerializeObject( Clients ) );
    }

    public override void SelectClientObjects( string args )
    {
      RhinoDoc.ActiveDoc.Objects.UnselectAll();
      var client = JsonConvert.DeserializeObject<dynamic>( args );

      // TODO: figure out what kind of filter this is, and "select" it. somehow.

      RhinoDoc.ActiveDoc.Views.Redraw();
    }

    public override string GetFileClients()
    {
      var strings = RhinoDoc.ActiveDoc.Strings.GetValue( "speckle" );
      try
      {
        Clients = JsonConvert.DeserializeObject<List<dynamic>>( strings );
      }
      catch( Exception e )
      {
        Clients = new List<dynamic>();
      }
      return strings;
    }

    public override string GetDocumentId()
    {
      return RhinoDoc.ActiveDoc.RuntimeSerialNumber.ToString();
    }

    public override string GetFileName()
    {
      return RhinoDoc.ActiveDoc.Name;
    }

    public override string GetDocumentLocation()
    {
      return RhinoDoc.ActiveDoc.Path;
    }

    public override string GetApplicationHostName()
    {
      return "Rhino";
    }

    public override List<ISelectionFilter> GetSelectionFilters()
    {
      return new List<ISelectionFilter>
      {
        new ElementsSelectionFilter
        {
          Name = "Selection",
          Icon = "mouse",
          Selection = new List<string>()
        }
      };
    }

  }
}