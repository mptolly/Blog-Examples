import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version, DisplayMode } from '@microsoft/sp-core-library';
import {
  BaseClientSideWebPart,
  IPropertyPaneConfiguration,
} from '@microsoft/sp-webpart-base';

import InlineHeaderExample from './components/InlineHeaderExample';
import { IInlineHeaderExampleProps } from './components/IInlineHeaderExampleProps';
import { IInlineHeaderExampleWebPartProps } from './IInlineHeaderExampleWebPartProps';

export default class InlineHeaderExampleWebPart extends BaseClientSideWebPart<IInlineHeaderExampleWebPartProps> {

  public render(): void {
    const props = this.properties;

    const element: React.ReactElement<IInlineHeaderExampleProps > = React.createElement(
      InlineHeaderExample,
      {
        title: this.properties.title,
        isEditMode: this.displayMode==DisplayMode.Edit,
        setTitle: function(title:string){
          props.title=title;
        }
      }
    );

    ReactDom.render(element, this.domElement);
  }

  protected get dataVersion(): Version {
    return Version.parse('1.0');
  }

  protected getPropertyPaneConfiguration(): IPropertyPaneConfiguration {
    return {
      pages: [
        {
          header: {
            description: ""
          },
          groups: []
        }
      ]
    };
  }
}
