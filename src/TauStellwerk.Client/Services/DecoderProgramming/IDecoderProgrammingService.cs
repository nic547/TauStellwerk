// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Base.Dto;

namespace TauStellwerk.Client.Services.DecoderProgramming;
public interface IDecoderProgrammingService
{
    public Task<ResultDto<int>> ReadDccAddress();

    public Task<ResultDto> WriteDccAddress(int address);
}
