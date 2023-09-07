export const escapeRegExp = (str: string) => {
  if (!str || str.length === 0) return "";
  return str.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
};
