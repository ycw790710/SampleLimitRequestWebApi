import React, { Fragment } from "react";
import classes from "./HighlightText.module.css";
import { escapeRegExp } from "./escapeRegExp";

interface HighlightTextProps {
  text: string;
  keyword: string;
}
const HighlightText: React.FC<HighlightTextProps> = ({ text, keyword }) => {
  const escapedKeyword = escapeRegExp(keyword);
  const regex = new RegExp(`(${escapedKeyword})`, "gi");
  const parts = text.split(regex);

  return (
    <Fragment>
      {keyword.length === 0 && <span>{text}</span>}
      {keyword.length > 0 &&
        parts.map((part, index) =>
          regex.test(part) ? (
            <span key={index} className={classes["highlight-text"]}>
              {part}
            </span>
          ) : (
            <span key={index}>{part}</span>
          )
        )}
    </Fragment>
  );
};

export default HighlightText;
