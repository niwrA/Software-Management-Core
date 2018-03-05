using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;

namespace LinksShared
{
  public abstract class LinkCommand : CommandBase, ICommand
  {
    public LinkCommand() : base() { }
    public LinkCommand(ICommandStateRepository repo) : base(repo) { }
    public virtual void Execute() { }
  }

  public class DeleteLinkCommand : LinkCommand
  {
    public override void Execute()
    {
      ((ILinkService)base.CommandProcessor).DeleteLink(this.EntityGuid);
      base.Execute();
    }

  }

  public class CreateLinkCommand : LinkCommand
  {
    public string Url { get; set; }
    public string Name { get; set; }
    public Guid ForGuid { get; set; }
    public override void Execute()
    {
      ((ILinkService)base.CommandProcessor).CreateLink(EntityGuid, ForGuid, Url, Name);
      base.Execute();
    }
  }

  public class RenameLinkCommand : LinkCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public override void Execute()
    {
      var product = ((ILinkService)base.CommandProcessor).GetLink(this.EntityGuid);
      product.Rename(this.Name, this.OriginalName);
      base.Execute();
    }
  }

  public class ChangeUrlForLinkCommand : LinkCommand
  {
    public string OriginalUrl { get; set; }
    public string Url { get; set; }
    public override void Execute()
    {
      var product = ((ILinkService)base.CommandProcessor).GetLink(this.EntityGuid);
      product.ChangeUrl(this.Url, this.OriginalUrl);
      base.Execute();
    }
  }


}
