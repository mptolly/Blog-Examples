import * as React from 'react';
import styles from './InlineHeaderExample.module.scss';
import * as strings from 'inlineHeaderExampleStrings';
import { IInlineHeaderExampleProps } from './IInlineHeaderExampleProps';

export default class InlineHeaderExample extends React.Component<IInlineHeaderExampleProps, void> {

  public setTitle(event){
    this.props.setTitle(event.target.value);
  }

  public render(): React.ReactElement<IInlineHeaderExampleProps> {
    return (
      <div>
        <div className={styles["webpart-header"]}>
          { this.props.isEditMode && <textarea onChange={this.setTitle.bind(this)} className={styles["edit"]} placeholder={strings.TitlePlaceholder} aria-label="Add a title" defaultValue={this.props.title}></textarea> }
          { !this.props.isEditMode && <span className={styles["view"]}>{this.props.title}</span> }          
        </div>
      </div>
    );
  }
}
